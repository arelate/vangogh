package cli

import (
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/nod"
	"net/url"
)

func LocalTagHandler(u *url.URL) error {
	ids, err := vangogh_local_data.IdsFromUrl(u)
	if err != nil {
		return err
	}

	return LocalTag(
		ids,
		vangogh_local_data.ValueFromUrl(u, "operation"),
		vangogh_local_data.ValueFromUrl(u, "tag-name"))
}

func LocalTag(ids []string, operation string, tagName string) error {

	lta := nod.NewProgress("%s local tag %s...", operation, tagName)
	defer lta.End()

	switch operation {
	case "add":
		if err := vangogh_local_data.AddLocalTags(ids, []string{tagName}, lta); err != nil {
			return err
		}
	case "remove":
		if err := vangogh_local_data.RemoveLocalTags(ids, []string{tagName}, lta); err != nil {
			return err
		}
	}

	lta.EndWithResult("done")

	return nil
}
