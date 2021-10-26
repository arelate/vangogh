package cli_api

import (
	"encoding/json"
	"fmt"
	"github.com/arelate/gog_types"
	"github.com/arelate/gog_urls"
	"github.com/arelate/vangogh_extracts"
	"github.com/arelate/vangogh_properties"
	"github.com/boggydigital/gost"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/vangogh/cli_api/http_client"
	"github.com/boggydigital/vangogh/cli_api/url_helpers"
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
	idSet, err := url_helpers.IdSet(u)
	if err != nil {
		return err
	}

	operation := url_helpers.Value(u, "operation")
	tagName := url_helpers.Value(u, "tag-name")

	return Tag(idSet, operation, tagName)
}

func Tag(idSet gost.StrSet, operation, tagName string) error {

	ta := nod.Begin("performing requested tag operation...")
	defer ta.End()

	//matching default GOG.com capitalization for tags
	tagName = strings.ToUpper(tagName)

	exl, err := vangogh_extracts.NewList(
		vangogh_properties.TagNameProperty,
		vangogh_properties.TagIdProperty,
		vangogh_properties.TitleProperty,
	)
	if err != nil {
		return err
	}

	tagId := ""
	if operation != createOp {
		tagId, err = tagIdByName(tagName, exl)
		if err != nil {
			return err
		}
	}

	switch operation {
	case createOp:
		return createTag(tagName, exl)
	case deleteOp:
		return deleteTag(tagName, tagId, exl)
	case addOp:
		return addTag(idSet, tagName, tagId, exl)
	case removeOp:
		return removeTag(idSet, tagName, tagId, exl)
	default:
		return ta.EndWithError(fmt.Errorf("vangogh: unknown tag operation %s", operation))
	}
}

func postResp(url *url.URL, respVal interface{}) error {
	httpClient, err := http_client.Default()
	if err != nil {
		return err
	}

	resp, err := httpClient.Post(url.String(), "", nil)
	if err != nil {
		return err
	}
	defer resp.Body.Close()

	if resp.StatusCode < 200 || resp.StatusCode > 299 {
		return fmt.Errorf("vangogh: unexpected status: %s", resp.Status)
	}

	return json.NewDecoder(resp.Body).Decode(&respVal)
}

func tagIdByName(tagName string, exl *vangogh_extracts.ExtractsList) (string, error) {
	if err := exl.AssertSupport(vangogh_properties.TagNameProperty); err != nil {
		return "", err
	}

	tagIds := exl.Search(map[string][]string{vangogh_properties.TagNameProperty: {tagName}}, true)
	if len(tagIds) == 0 {
		return "", fmt.Errorf("vangogh: unknown tag-name %s", tagName)
	}
	if len(tagIds) > 1 {
		return "", fmt.Errorf("vangogh: ambiguous tag-name %s, matching tag-ids: %s",
			tagName,
			strings.Join(tagIds, ","))
	}
	return tagIds[0], nil
}

func createTag(tagName string, exl *vangogh_extracts.ExtractsList) error {

	cta := nod.Begin(" creating tag %s...", tagName)
	defer cta.End()

	if err := exl.AssertSupport(vangogh_properties.TagNameProperty); err != nil {
		return cta.EndWithError(err)
	}

	createTagUrl := gog_urls.CreateTag(tagName)
	var ctResp gog_types.CreateTagResp
	if err := postResp(createTagUrl, &ctResp); err != nil {
		return cta.EndWithError(err)
	}
	if ctResp.Id == "" {
		return cta.EndWithError(fmt.Errorf("invalid create tag response"))
	}

	if err := exl.Add(vangogh_properties.TagNameProperty, ctResp.Id, tagName); err != nil {
		return cta.EndWithError(err)
	}

	cta.EndWithResult("done")

	return nil
}

func deleteTag(tagName, tagId string, exl *vangogh_extracts.ExtractsList) error {

	dta := nod.Begin(" deleting tag %s...", tagName)
	defer dta.End()

	if err := exl.AssertSupport(vangogh_properties.TagNameProperty); err != nil {
		return dta.EndWithError(err)
	}

	deleteTagUrl := gog_urls.DeleteTag(tagId)
	var dtResp gog_types.DeleteTagResp
	if err := postResp(deleteTagUrl, &dtResp); err != nil {
		return dta.EndWithError(err)
	}
	if dtResp.Status != "deleted" {
		return dta.EndWithError(fmt.Errorf("invalid delete tag response"))
	}

	if err := exl.Remove(vangogh_properties.TagNameProperty, tagId, tagName); err != nil {
		return dta.EndWithError(err)
	}

	dta.EndWithResult("done")

	return nil
}

func addTag(idSet gost.StrSet, tagName, tagId string, exl *vangogh_extracts.ExtractsList) error {

	ata := nod.NewProgress(" adding tag %s to item(s)...", tagName)
	defer ata.End()

	if err := exl.AssertSupport(vangogh_properties.TagNameProperty, vangogh_properties.TitleProperty); err != nil {
		return ata.EndWithError(err)
	}

	ata.TotalInt(idSet.Len())

	for _, id := range idSet.All() {
		addTagUrl := gog_urls.AddTag(id, tagId)
		var artResp gog_types.AddRemoveTagResp
		if err := postResp(addTagUrl, &artResp); err != nil {
			return ata.EndWithError(err)
		}
		if !artResp.Success {
			return ata.EndWithError(fmt.Errorf("vangogh: failed to add tag %s", tagName))
		}

		if err := exl.Add(vangogh_properties.TagIdProperty, id, tagId); err != nil {
			return ata.EndWithError(err)
		}

		ata.Increment()
	}

	ata.EndWithResult("done")

	return nil
}

func removeTag(idSet gost.StrSet, tagName, tagId string, exl *vangogh_extracts.ExtractsList) error {

	rta := nod.NewProgress(" removing tag %s from item(s)...", tagName)
	defer rta.End()

	if err := exl.AssertSupport(vangogh_properties.TagNameProperty, vangogh_properties.TitleProperty); err != nil {
		return rta.EndWithError(err)
	}

	rta.TotalInt(idSet.Len())

	for _, id := range idSet.All() {
		removeTagUrl := gog_urls.RemoveTag(id, tagId)
		var artResp gog_types.AddRemoveTagResp
		if err := postResp(removeTagUrl, &artResp); err != nil {
			return rta.EndWithError(err)
		}
		if !artResp.Success {
			return rta.EndWithError(fmt.Errorf("vangogh: failed to remove tag %s", tagName))
		}

		if err := exl.Remove(vangogh_properties.TagIdProperty, id, tagId); err != nil {
			return rta.EndWithError(err)
		}

		rta.Increment()
	}

	rta.EndWithResult("done")

	return nil
}
