package gog_data

import (
	"github.com/arelate/southern_light/gog_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/cli/fetch"
	"github.com/arelate/vangogh/cli/reqs"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"net/http"
)

func GetUserAccessToken(hc *http.Client) error {
	guata := nod.Begin("getting %s...", vangogh_integration.UserAccessToken)
	defer guata.Done()

	userAccessTokenDir, err := vangogh_integration.AbsProductTypeDir(vangogh_integration.UserAccessToken)
	if err != nil {
		return err
	}

	kvUserAccessToken, err := kevlar.New(userAccessTokenDir, kevlar.JsonExt)
	if err != nil {
		return err
	}

	uatId := vangogh_integration.UserAccessToken.String()

	if err = fetch.SetValue(uatId,
		gog_integration.UserAccessTokenUrl(),
		reqs.UserAccessToken(hc),
		kvUserAccessToken); err != nil {
		return err
	}

	return nil
}
