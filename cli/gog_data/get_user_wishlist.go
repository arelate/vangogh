package gog_data

import (
	"encoding/json"
	"github.com/arelate/southern_light/gog_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/cli/fetch"
	"github.com/arelate/vangogh/cli/reqs"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/pathways"
	"github.com/boggydigital/redux"
	"net/http"
	"slices"
)

func GetUserWishlist(hc *http.Client, uat string) error {
	guwa := nod.Begin("getting %s...", vangogh_integration.UserWishlist)
	defer guwa.Done()

	userWishlistDir, err := vangogh_integration.AbsProductTypeDir(vangogh_integration.UserWishlist)
	if err != nil {
		return err
	}

	kvUserWishlist, err := kevlar.New(userWishlistDir, kevlar.JsonExt)
	if err != nil {
		return err
	}

	userWishlistId := vangogh_integration.UserWishlist.String()

	if err = fetch.SetValue(userWishlistId,
		gog_integration.UserWishlistUrl(),
		reqs.UserWishlist(hc, uat),
		kvUserWishlist); err != nil {
		return err

	}

	return reduceUserWishlist(kvUserWishlist)
}

func reduceUserWishlist(kvUserWishlist kevlar.KeyValues) error {

	ruwa := nod.Begin(" reducing %s...", vangogh_integration.UserWishlist)
	defer ruwa.Done()

	reduxDir, err := pathways.GetAbsRelDir(vangogh_integration.Redux)
	if err != nil {
		return err
	}

	key := vangogh_integration.UserWishlistProperty

	rdx, err := redux.NewWriter(reduxDir, key)
	if err != nil {
		return err
	}

	rcUserWishlist, err := kvUserWishlist.Get(vangogh_integration.UserWishlist.String())
	if err != nil {
		return err
	}
	defer rcUserWishlist.Close()

	var userWishlist gog_integration.UserWishlist
	if err = json.NewDecoder(rcUserWishlist).Decode(&userWishlist); err != nil {
		return err
	}

	userWishlistMap := make(map[string][]string, len(userWishlist.Wishlist))
	for id, flag := range userWishlist.Wishlist {
		if flag {
			userWishlistMap[id] = []string{vangogh_integration.TrueValue}
		}
	}

	if err = rdx.CutKeys(key, slices.Collect(rdx.Keys(key))...); err != nil {
		return err
	}

	return rdx.BatchAddValues(key, userWishlistMap)
}
