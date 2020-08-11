package pages

type Page struct {
	Page             int    `json:"page"`
	TotalPages       int    `json:"totalPages"`
	TotalResults     string `json:"totalResults"`
	TotalGamesFound  int    `json:"totalGamesFound"`
	TotalMoviesFound int    `json:"totalMoviesFound"`
}
