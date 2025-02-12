package cli

import (
	"errors"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/coost"
	"github.com/boggydigital/nod"
	"net/url"
	"strings"
)

const (
	createOp = "create"
	deleteOp = "delete"
	addOp    = "add"
	removeOp = "remove"
)

func TagHandler(u *url.URL) error {
	ids, err := vangogh_integration.IdsFromUrl(u)
	if err != nil {
		return err
	}

	return Tag(
		ids,
		vangogh_integration.ValueFromUrl(u, "operation"),
		vangogh_integration.ValueFromUrl(u, "tag-name"))
}

func Tag(ids []string, operation, tagName string) error {

	ta := nod.Begin("performing requested tag operation...")
	defer ta.EndWithResult("done")

	//matching default GOG.com capitalization for tags
	tagName = strings.ToUpper(tagName)

	var err error
	tagId := ""

	if operation != createOp {
		tagId, err = vangogh_integration.TagIdByName(tagName)
		if err != nil {
			return err
		}
	}

	acp, err := vangogh_integration.AbsCookiePath()
	if err != nil {
		return err
	}

	hc, err := coost.NewHttpClientFromFile(acp)
	if err != nil {
		return err
	}

	toa := nod.NewProgress(" %s tag %s...", operation, tagName)
	defer toa.EndWithResult("done")

	switch operation {
	case createOp:
		return vangogh_integration.CreateTag(hc, tagName)
	case deleteOp:
		return vangogh_integration.DeleteTag(hc, tagName, tagId)
	case addOp:
		return vangogh_integration.AddTags(hc, ids, []string{tagId}, toa)
	case removeOp:
		return vangogh_integration.RemoveTags(hc, ids, []string{tagId}, toa)
	default:
		return errors.New("unknown tag operation " + operation)
	}
}
