package rest

func RefreshReduxAssets(properties ...string) (err error) {
	if rdx, err = rdx.RefreshReader(); err != nil {
		return err
	}
	if err := rdx.MustHave(properties...); err != nil {
		return err
	}
	return err
}
