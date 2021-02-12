package cmd

import (
	"bytes"
	"encoding/json"
	"fmt"
	"github.com/arelate/gogtypes"
	"github.com/boggydigital/kvas"
	"io"
	"log"
	"strconv"
)

func split(mainPt string, media string) error {
	mainDstUrl, err := destinationUrl(mainPt, media)
	if err != nil {
		return err
	}

	kvMain, err := kvas.NewLocal(mainDstUrl, jsonExt, jsonExt)
	if err != nil {
		return err
	}

	for _, pp := range kvMain.All() {

		log.Printf("splitting %s (%s) page %s\n", mainPt, media, pp)

		pageRc, err := kvMain.Get(pp)
		if err != nil {
			return err
		}

		if err := splitPage(pageRc, mainPt, media); err != nil {
			return err
		}

		if err := pageRc.Close(); err != nil {
			return err
		}
	}

	return nil
}

func splitPage(pageReader io.Reader, mainPt string, media string) error {

	detailPt := detailProductType(mainPt)

	detailDstUrl, err := destinationUrl(detailPt, media)
	if err != nil {
		return nil
	}

	kvDetail, err := kvas.NewLocal(detailDstUrl, jsonExt, jsonExt)
	if err != nil {
		return err
	}

	switch mainPt {
	case Store:
		return splitProductsPage(pageReader, kvDetail.Set)
	case Account:
		return splitAccountProductsPage(pageReader, kvDetail.Set)
	case Wishlist:
		return splitWishlistPage(pageReader, kvDetail.Set)
	default:
		return fmt.Errorf("splitting page is not supported for type %s", mainPt)
	}
}

// TODO: rewrite this with generics when available
func splitProductsPage(pageReader io.Reader, set func(string, io.Reader) error) error {
	var productsPage gogtypes.ProductsPage
	if err := json.NewDecoder(pageReader).Decode(&productsPage); err != nil {
		return err
	}

	for _, product := range productsPage.Products {
		if err := setEncoded(product.Id, product, set); err != nil {
			return err
		}
	}

	return nil
}

// TODO: rewrite this with generics when available
func splitAccountProductsPage(pageReader io.Reader, set func(string, io.Reader) error) error {
	var accountProductsPage gogtypes.AccountProductsPage
	if err := json.NewDecoder(pageReader).Decode(&accountProductsPage); err != nil {
		return err
	}

	for _, accountProduct := range accountProductsPage.Products {
		if err := setEncoded(accountProduct.Id, accountProduct, set); err != nil {
			return err
		}
	}

	return nil
}

// TODO: rewrite this with generics when available
func splitWishlistPage(pageReader io.Reader, set func(string, io.Reader) error) error {
	var wishlistPage gogtypes.WishlistPage
	if err := json.NewDecoder(pageReader).Decode(&wishlistPage); err != nil {
		return err
	}

	for _, wishlistProduct := range wishlistPage.Products {
		if err := setEncoded(wishlistProduct.Id, wishlistProduct, set); err != nil {
			return err
		}
	}

	return nil
}

func setEncoded(id int, item interface{}, set func(string, io.Reader) error) error {
	var buf bytes.Buffer

	if err := json.NewEncoder(&buf).Encode(item); err != nil {
		return err
	}

	if err := set(strconv.Itoa(id), &buf); err != nil {
		return err
	}

	return nil
}
