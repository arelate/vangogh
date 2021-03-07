package cmd

import (
	"bytes"
	"encoding/json"
	"fmt"
	"github.com/arelate/gog_types"
	"github.com/arelate/vangogh_types"
	"github.com/arelate/vangogh_urls"
	"github.com/boggydigital/kvas"
	"io"
	"log"
	"strconv"
)

func split(mainPt vangogh_types.ProductType, mt gog_types.Media) error {

	mainDstUrl, err := vangogh_urls.DstProductTypeUrl(mainPt, mt)
	if err != nil {
		return err
	}

	kvMain, err := kvas.NewJsonLocal(mainDstUrl)
	if err != nil {
		return err
	}

	for _, pp := range kvMain.All() {

		log.Printf("splitting %s (%s) page %s\n", mainPt, mt, pp)

		pageRc, err := kvMain.Get(pp)
		if err != nil {
			return err
		}

		if err := splitPage(pageRc, mainPt, mt); err != nil {
			return err
		}

		if err := pageRc.Close(); err != nil {
			return err
		}
	}

	return nil
}

func splitPage(pageReader io.Reader, mainPt vangogh_types.ProductType, mt gog_types.Media) error {

	detailPt := vangogh_types.DetailProductType(mainPt)

	detailDstUrl, err := vangogh_urls.DstProductTypeUrl(detailPt, mt)
	if err != nil {
		return nil
	}

	kvDetail, err := kvas.NewJsonLocal(detailDstUrl)
	if err != nil {
		return err
	}

	switch mainPt {
	case vangogh_types.StorePage:
		return splitProductsPage(pageReader, kvDetail.Set)
	case vangogh_types.AccountPage:
		return splitAccountProductsPage(pageReader, kvDetail.Set)
	case vangogh_types.WishlistPage:
		return splitWishlistPage(pageReader, kvDetail.Set)
	default:
		return fmt.Errorf("splitting page is not supported for type %s", mainPt)
	}
}

// TODO: rewrite this with generics when available
func splitProductsPage(pageReader io.Reader, set func(string, io.Reader) error) error {
	var storeProductsPage gog_types.StoreProductsPage
	if err := json.NewDecoder(pageReader).Decode(&storeProductsPage); err != nil {
		return err
	}

	for _, product := range storeProductsPage.Products {
		if err := setEncoded(product.Id, product, set); err != nil {
			return err
		}
	}

	return nil
}

// TODO: rewrite this with generics when available
func splitAccountProductsPage(pageReader io.Reader, set func(string, io.Reader) error) error {
	var accountProductsPage gog_types.AccountProductsPage
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
	var wishlistPage gog_types.WishlistPage
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
