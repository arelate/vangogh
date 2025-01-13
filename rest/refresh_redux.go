package rest

import "github.com/arelate/southern_light/vangogh_integration"

func RefreshRedux() (err error) {
	if rdx, err = rdx.RefreshReader(); err != nil {
		return err
	}
	if err := rdx.MustHave(vangogh_integration.AllProperties()...); err != nil {
		return err
	}
	return err
}
