package products

import (
	"encoding/json"
	"fmt"
	"github.com/boggydigital/vangogh/internal/gog/urls"
	"io/ioutil"
	"net/http"
)

const (
	MediaTypeGame  = "game"
	MediaTypeMovie = "movie"
)

func fetchPage(client *http.Client, mediaType string, page int) (*ProductPage, error) {
	resp, _ := client.Get(urls.ProductsPageURL(mediaType, page).String())
	defer resp.Body.Close()

	respBody, err := ioutil.ReadAll(resp.Body)
	if err != nil {
		return &ProductPage{}, err
	}

	var productPage ProductPage

	err = json.Unmarshal(respBody, &productPage)
	if err != nil {
		return &ProductPage{}, err
	}

	return &productPage, nil

}

func Fetch(client *http.Client, mediaType string) (*[]Product, error) {
	var products []Product
	firstPage, err := fetchPage(client, mediaType, 1)
	fmt.Println("Fetched first page...")
	if err != nil {
		return nil, err
	}
	products = append(products, firstPage.Products...)

	// TODO: obviously we'd want to do that concurrently
	for p := 2; p < firstPage.TotalPages; p++ {
		page, err := fetchPage(client, mediaType, p)
		if err != nil {
			return &products, err
		}
		products = append(products, page.Products...)
		fmt.Printf("Fetched page %d out of %d\n", p, firstPage.TotalPages)
	}

	return &products, nil
}
