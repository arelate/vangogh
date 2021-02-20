package cmd

func Sync(media string) error {
	//sync paginated product types
	productTypes := []string{Store, Account, Wishlist}
	for _, pt := range productTypes {
		if err := Fetch(nil, pt, media, false); err != nil {
			return err
		}
	}
	// sync main - detail missing product types
	productTypes = []string{Details, ApiProducts}
	for _, pt := range productTypes {
		if err := Fetch(nil, pt, media, true); err != nil {
			return err
		}
	}
	return nil
}
