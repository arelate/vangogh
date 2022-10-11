package cli

import (
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/nod"
	"golang.org/x/exp/maps"
	"net/url"
)

func LocalTagHandler(u *url.URL) error {
	idSet, err := vangogh_local_data.IdSetFromUrl(u)
	if err != nil {
		return err
	}

	return LocalTag(
		idSet,
		vangogh_local_data.ValueFromUrl(u, "operation"),
		vangogh_local_data.ValueFromUrl(u, "tag-name"))
}

func LocalTag(idSet map[string]bool, operation string, tagName string) error {

	lta := nod.NewProgress("%s local tag %s...", operation, tagName)
	defer lta.End()

	switch operation {
	case "add":
		if err := vangogh_local_data.AddLocalTags(maps.Keys(idSet), []string{tagName}, lta); err != nil {
			return err
		}
	case "remove":
		if err := vangogh_local_data.RemoveLocalTags(maps.Keys(idSet), []string{tagName}, lta); err != nil {
			return err
		}
	}

	lta.EndWithResult("done")

	return nil
}
