package cmd

func Sync(media string) error {
	// sync paginated product types
	productTypes := []string{"products", "account-products", "wishlist"}
	for _, pt := range productTypes {
		if err := Fetch(nil, pt, media, false); err != nil {
			return err
		}
	}
	// sync missing details
	if err := Fetch(nil, "details", media, true); err != nil {
		return err
	}
	return nil
}
