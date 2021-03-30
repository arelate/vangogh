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

func Wishlist(mt gog_media.Media, add, remove []string) error {
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
		"add",
		"to",
		add,
		gog_urls.AddToWishlist,
		httpClient,
		vrStoreProducts,
		titleExtracts); err != nil {
		return err
	}

	if err := wishlistCommand(
		"remove",
		"from",
		remove,
		gog_urls.RemoveFromWishlist,
		httpClient,
		vrStoreProducts,
		titleExtracts); err != nil {
		return err
	}

	return nil
}

func wishlistCommand(
	op, dir string,
	ids []string,
	wishlistUrl func(string) *url.URL,
	httpClient *http.Client,
	vr *vangogh_values.ValueReader,
	titleExtracts *froth.Stash) error {
	for _, id := range ids {
		if !vr.Contains(id) {
			// TODO: log
			log.Printf("vangogh: can't %s invalid id %s %s the wishlist", op, id, dir)
			continue
		}
		title, ok := titleExtracts.Get(id)
		if !ok {
			title = id
		}
		fmt.Printf("%s %s (%s) %s wishlist\n", op, title, id, dir)
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
