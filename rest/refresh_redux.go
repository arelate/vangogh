package rest

import "github.com/arelate/vangogh_local_data"

func RefreshRedux() (err error) {
	if rdx, err = rdx.RefreshReader(); err != nil {
		return err
	}
	if err := rdx.MustHave(vangogh_local_data.AllProperties()...); err != nil {
		return err
	}
	return err
}
