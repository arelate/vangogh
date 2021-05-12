package cmd

import (
	"fmt"
	"github.com/arelate/gog_media"
	"github.com/arelate/vangogh_products"
	"time"
)

func Summary(since int64, mt gog_media.Media) error {

	fmt.Printf("changes since %s:\n", time.Unix(since, 0).Format(time.Kitchen))
	for _, pt := range vangogh_products.Local() {
		fmt.Printf("new or updated %s (%s) during this sync:\n", pt, mt)
		if err := List(nil, since, pt, mt); err != nil {
			return err
		}
	}

	return nil
}
