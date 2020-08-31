package cmd

import (
	"errors"
	"fmt"
	"github.com/boggydigital/vangogh/internal/gog/media"
	"github.com/boggydigital/vangogh/internal/mongocl"
	"time"
)

type fetchPage func(deps FetchDeps, page int) (totalPages int, err error)

func fetchPages(deps FetchDeps, fetchPage fetchPage) error {

	if deps.Media == media.Unknown {
		return errors.New(fmt.Sprintf("cannot fetch %s of type 'unknown'", deps.Product))
	}

	if deps.Media == media.All {
		deps.Media = media.Game
		if err := fetchPages(deps, fetchPage); err != nil {
			return err
		}
		deps.Media = media.Movie
		return fetchPages(deps, fetchPage)
	}

	start := time.Now().Unix()
	totalPages := 1

	fmt.Printf("Fetching %s of type %s.\n", deps.Product, deps.Media.String())
	for page := 1; page <= totalPages; page++ {

		fmt.Printf("Page %d/%d", page, totalPages)
		tp, err := fetchPage(deps, page)
		if err != nil {
			return err
		}
		totalPages = tp
		fmt.Println()
	}

	ad, mod, err := mongocl.ChangedSince(deps.MongoClient, deps.Ctx, deps.Collection, start)
	if err != nil {
		return err
	}
	fmt.Printf("Added %d, modified %d %s of type %s.\n", len(ad), len(mod), deps.Product, deps.Media)

	return nil
}
