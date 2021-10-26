package cli_api

import (
	"github.com/arelate/gog_media"
	"github.com/arelate/gog_urls"
	"github.com/arelate/vangogh_products"
	"github.com/arelate/vangogh_values"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/vangogh/cli_api/http_client"
	"github.com/boggydigital/vangogh/cli_api/remove"
	"github.com/boggydigital/vangogh/cli_api/url_helpers"
	"net/http"
	"net/url"
)

func WishlistHandler(u *url.URL) error {
	mt := gog_media.Parse(url_helpers.Value(u, "media"))

	addProductIds := url_helpers.Values(u, "add")
	removeProductIds := url_helpers.Values(u, "remove")

	return Wishlist(mt, addProductIds, removeProductIds)
}

func Wishlist(mt gog_media.Media, addProductIds, removeProductIds []string) error {

	wa := nod.Begin("performing requested wishlist operations...")
	defer wa.End()

	httpClient, err := http_client.Default()
	if err != nil {
		return wa.EndWithError(err)
	}

	vrStoreProducts, err := vangogh_values.NewReader(vangogh_products.StoreProducts, mt)
	if err != nil {
		return wa.EndWithError(err)
	}

	if len(addProductIds) > 0 {
		if err := wishlistAdd(addProductIds, httpClient, vrStoreProducts, mt); err != nil {
			return wa.EndWithError(err)
		}
	}

	if len(removeProductIds) > 0 {
		if err := wishlistRemove(removeProductIds, httpClient, vrStoreProducts, mt); err != nil {
			return wa.EndWithError(err)
		}
	}

	wa.EndWithResult("done")

	return nil
}

func wishlistAdd(
	ids []string,
	httpClient *http.Client,
	vrStoreProducts *vangogh_values.ValueReader,
	mt gog_media.Media) error {

	waa := nod.NewProgress(" adding product(s) to local wishlist...")
	defer waa.End()

	waa.TotalInt(len(ids))

	for _, id := range ids {
		if err := vrStoreProducts.CopyToType(id, vangogh_products.WishlistProducts, mt); err != nil {
			return waa.EndWithError(err)
		}
		waa.Increment()
	}

	waa.EndWithResult("done")

	return remoteWishlistCommand(
		ids,
		gog_urls.AddToWishlist,
		httpClient,
		vrStoreProducts)
}

func wishlistRemove(
	ids []string,
	httpClient *http.Client,
	vrStoreProducts *vangogh_values.ValueReader,
	mt gog_media.Media) error {

	wra := nod.NewProgress(" removing product(s) from local wishlist...")
	defer wra.End()

	if err := remove.Data(ids, vangogh_products.WishlistProducts, mt); err != nil {
		return wra.EndWithError(err)
	}

	wra.EndWithResult("done")

	return remoteWishlistCommand(
		ids,
		gog_urls.RemoveFromWishlist,
		httpClient,
		vrStoreProducts)
}

func remoteWishlistCommand(
	ids []string,
	wishlistUrl func(string) *url.URL,
	httpClient *http.Client,
	vrStoreProducts *vangogh_values.ValueReader) error {

	rwca := nod.NewProgress(" syncing to remote wishlist...")
	defer rwca.End()

	rwca.TotalInt(len(ids))

	for _, id := range ids {
		if !vrStoreProducts.Contains(id) {
			continue
		}
		wUrl := wishlistUrl(id)
		resp, err := httpClient.Get(wUrl.String())
		if err != nil {
			return rwca.EndWithError(err)
		}

		if err := resp.Body.Close(); err != nil {
			return rwca.EndWithError(err)
		}

		rwca.Increment()
	}

	rwca.EndWithResult("done")

	return nil
}
