package cli

import (
	"fmt"
	"github.com/arelate/vangogh_local_data"
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
	ids, err := vangogh_local_data.IdsFromUrl(u)
	if err != nil {
		return err
	}

	return Tag(
		ids,
		vangogh_local_data.ValueFromUrl(u, "operation"),
		vangogh_local_data.ValueFromUrl(u, "tag-name"))
}

func Tag(ids []string, operation, tagName string) error {

	ta := nod.Begin("performing requested tag operation...")
	defer ta.End()

	//matching default GOG.com capitalization for tags
	tagName = strings.ToUpper(tagName)

	var err error
	tagId := ""

	if operation != createOp {
		tagId, err = vangogh_local_data.TagIdByName(tagName)
		if err != nil {
			return err
		}
	}

	acp, err := vangogh_local_data.AbsCookiePath()
	if err != nil {
		return ta.EndWithError(err)
	}

	hc, err := coost.NewHttpClientFromFile(acp)
	if err != nil {
		return ta.EndWithError(err)
	}

	toa := nod.NewProgress(" %s tag %s...", operation, tagName)
	defer toa.End()

	switch operation {
	case createOp:
		return vangogh_local_data.CreateTag(hc, tagName)
	case deleteOp:
		return vangogh_local_data.DeleteTag(hc, tagName, tagId)
	case addOp:
		return vangogh_local_data.AddTags(hc, ids, []string{tagId}, toa)
	case removeOp:
		return vangogh_local_data.RemoveTags(hc, ids, []string{tagId}, toa)
	default:
		return ta.EndWithError(fmt.Errorf("unknown tag operation %s", operation))
	}
}
