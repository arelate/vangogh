package cmd

import (
	"fmt"
	"github.com/arelate/gog_media"
	"github.com/arelate/gog_urls"
	"github.com/arelate/vangogh_products"
	"github.com/arelate/vangogh_properties"
	"github.com/arelate/vangogh_urls"
	"github.com/arelate/vangogh_values"
	"github.com/boggydigital/froth"
	"github.com/boggydigital/vangogh/internal"
	"log"
	"net/http"
	"net/url"
)

func Wishlist(mt gog_media.Media, addProductIds, removeProductIds []string) error {
	httpClient, err := internal.HttpClient()
	if err != nil {
		return err
	}

	vrStoreProducts, err := vangogh_values.NewReader(vangogh_products.StoreProducts, mt)
	if err != nil {
		return err
	}

	titleExtracts, err := froth.NewStash(
		vangogh_urls.ExtractsDir(),
		vangogh_properties.TitleProperty)
	if err != nil {
		return err
	}

	if err := wishlistCommand(
		addProductIds,
		gog_urls.AddToWishlist,
		addError,
		addProgress,
		httpClient,
		vrStoreProducts,
		titleExtracts); err != nil {
		return err
	}

	if err := wishlistCommand(
		removeProductIds,
		gog_urls.RemoveFromWishlist,
		removeError,
		removeProgress,
		httpClient,
		vrStoreProducts,
		titleExtracts); err != nil {
		return err
	}

	return nil
}

func addError(id string) string {
	return fmt.Sprintf("can't add invalid id %s to the wishlist", id)
}

func removeError(id string) string {
	return fmt.Sprintf("can't remove invalid id %s from the wishlist", id)
}

func addProgress(title, id string) string {
	return fmt.Sprintf("add %s (%s) to the wishlist", title, id)
}

func removeProgress(title, id string) string {
	return fmt.Sprintf("remove %s (%s) from the wishlist", title, id)
}

func wishlistCommand(
	ids []string,
	wishlistUrl func(string) *url.URL,
	fmtError func(string) string,
	fmtProgress func(string, string) string,
	httpClient *http.Client,
	vr *vangogh_values.ValueReader,
	titleExtracts *froth.Stash) error {
	for _, id := range ids {
		if !vr.Contains(id) {
			log.Printf("vangogh: %s", fmtError(id))
			continue
		}
		title, ok := titleExtracts.Get(id)
		if !ok {
			title = id
		}
		fmt.Println(fmtProgress(title, id))
		addUrl := wishlistUrl(id)
		resp, err := httpClient.Get(addUrl.String())
		if err != nil {
			return err
		}

		if err := resp.Body.Close(); err != nil {
			return err
		}
	}

	return nil
}
