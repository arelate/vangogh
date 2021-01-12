package cmd

import "fmt"

func Fetch(productType string, media string) error {
	fmt.Println("fetch ", productType, media)
	return nil
}
