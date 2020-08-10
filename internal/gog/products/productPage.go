package products

type ProductPage struct {
	Products         []Product   `json:"products"`
	Ts               interface{} `json:"ts"`
	Page             int         `json:"page"`
	TotalPages       int         `json:"totalPages"`
	TotalResults     string      `json:"totalResults"`
	TotalGamesFound  int         `json:"totalGamesFound"`
	TotalMoviesFound int         `json:"totalMoviesFound"`
}
