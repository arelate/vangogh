package schema

type ProductPage struct {
	Page
	Ts       interface{} `json:"ts"`
	Products []Product   `json:"products"`
}
