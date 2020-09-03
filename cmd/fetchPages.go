package cmd

import (
	"errors"
	"fmt"
	"github.com/boggydigital/vangogh/internal/gog/media"
	"github.com/boggydigital/vangogh/internal/mongocl"
	"log"
	"time"
)

type fetchPage func(deps FetchDeps, page int) (totalPages int, err error)

func fetchPages(deps FetchDeps, fetchPage fetchPage) error {

	log.Printf("fetchPages(%s,%s,%s)\n", deps.Media.String(), deps.Product, deps.Collection)

	if deps.Media == media.Unknown {
		return errors.New(fmt.Sprintf("cannot fetch %s of type 'unknown'", deps.Product))
	}

	mtypes := make([]media.Type, 0)

	if deps.Media != media.All {
		mtypes = append(mtypes, deps.Media)
	} else {
		mtypes = append(mtypes, media.Game, media.Movie)
	}

	start := time.Now().Unix()

	for _, mt := range mtypes {

		totalPages := 1

		fmt.Printf("Fetching %s of type %s.\n", deps.Product, mt.String())
		for page := 1; page <= totalPages; page++ {

			fmt.Printf("Page %d/%d", page, totalPages)

			deps.Media = mt
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
		fmt.Printf("Added %d, modified %d %s of type %s.\n", len(ad), len(mod), deps.Product, mt.String())
	}

	return nil
}
