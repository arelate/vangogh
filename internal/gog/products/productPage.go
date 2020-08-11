package products

import "github.com/boggydigital/vangogh/internal/gog/pages"

type ProductPage struct {
	pages.Page
	Ts       interface{} `json:"ts"`
	Products []Product   `json:"products"`
}
