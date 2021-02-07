package cmd

import (
	"encoding/json"
	"fmt"
	"github.com/arelate/gogtypes"
	"github.com/arelate/gogurls"
	"github.com/boggydigital/kvas"
	"io/ioutil"
	"log"
	"net/http"
	"strconv"
)

func Fetch(productType string, media string) error {
	fmt.Println("fetch ", productType, media)

	loc := "data"

	switch productType {
	case "products":
		loc += "/productPages"
	case "account-products":
		loc += "/accountProductPages"
	case "wishlist":
		loc += "/wishlistPages"
	case "details":
		loc += "/details"
	default:

	}

	totalPages := 1
	for pp := 1; pp <= totalPages; pp++ {

		fmt.Printf("fetching %s page %d/%d\n", productType, pp, totalPages)

		pageUrl := gogurls.ProductsPage(pp, gogtypes.Game, gogtypes.ProductsSortByNewestFirst)

		resp, err := http.Get(pageUrl.String())
		if err != nil {
			log.Fatal(err)
		}

		bytes, err := ioutil.ReadAll(resp.Body)
		if err != nil {
			log.Fatal(err)
		}

		vs, err := kvas.NewClient(loc, ".json", false)
		if err != nil {
			log.Fatal(err)
		}

		if err = vs.Set(strconv.Itoa(pp), bytes); err != nil {
			log.Fatal(err)
		}

		var page gogtypes.Page
		if err = json.Unmarshal(bytes, &page); err != nil {
			log.Fatal(err)
		}

		totalPages = page.TotalPages
	}

	return nil
}
