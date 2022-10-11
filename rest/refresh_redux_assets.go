package rest

func RefreshReduxAssets(properties ...string) (err error) {
	if rxa, err = rxa.RefreshReduxAssets(); err != nil {
		return err
	}
	if err := rxa.IsSupported(properties...); err != nil {
		return err
	}
	return err
}
