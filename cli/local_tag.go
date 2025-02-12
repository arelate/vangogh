package cli

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/nod"
	"net/url"
)

func LocalTagHandler(u *url.URL) error {
	ids, err := vangogh_integration.IdsFromUrl(u)
	if err != nil {
		return err
	}

	return LocalTag(
		ids,
		vangogh_integration.ValueFromUrl(u, "operation"),
		vangogh_integration.ValueFromUrl(u, "tag-name"))
}

func LocalTag(ids []string, operation string, tagName string) error {

	lta := nod.NewProgress("%s local tag %s...", operation, tagName)
	defer lta.EndWithResult("done")

	switch operation {
	case "add":
		if err := vangogh_integration.AddLocalTags(ids, []string{tagName}, lta); err != nil {
			return err
		}
	case "remove":
		if err := vangogh_integration.RemoveLocalTags(ids, []string{tagName}, lta); err != nil {
			return err
		}
	}

	return nil
}
