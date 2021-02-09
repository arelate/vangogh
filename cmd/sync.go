package cmd

func Sync(media string) error {
	productTypes := []string{"products", "account-products", "wishlist"}
	for _, pt := range productTypes {
		if err := Fetch(nil, pt, media, false); err != nil {
			return err
		}
	}
	return nil
}
