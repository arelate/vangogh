package cmd

import (
	"encoding/json"
	"fmt"
	"github.com/arelate/gog_types"
	"github.com/arelate/gog_urls"
	"github.com/arelate/vangogh_extracts"
	"github.com/arelate/vangogh_properties"
	"github.com/boggydigital/vangogh/internal"
	"net/url"
	"strings"
)

const (
	createOp = "create"
	deleteOp = "delete"
	addOp    = "add"
	removeOp = "remove"
)

func postResp(url *url.URL, respVal interface{}) error {
	httpClient, err := internal.HttpClient()
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

	createTagUrl := gog_urls.CreateTag(tagName)
	var ctResp gog_types.CreateTagResp
	if err := postResp(createTagUrl, &ctResp); err != nil {
		return err
	}
	if ctResp.Id == "" {
		return fmt.Errorf("vangogh: invalid create tag response")
	}

	if err := exl.Add(vangogh_properties.TagNameProperty, ctResp.Id, tagName); err != nil {
		return err
	}
	fmt.Printf("created tag %s\n", tagName)

	return nil
}

func deleteTag(tagName, tagId string, exl *vangogh_extracts.ExtractsList) error {
	deleteTagUrl := gog_urls.DeleteTag(tagId)
	var dtResp gog_types.DeleteTagResp
	if err := postResp(deleteTagUrl, &dtResp); err != nil {
		return err
	}
	if dtResp.Status != "deleted" {
		return fmt.Errorf("vangogh: invalid delete tag response")
	}

	if err := exl.Remove(vangogh_properties.TagNameProperty, tagId, tagName); err != nil {
		return err
	}
	fmt.Printf("deleted tag %s\n", tagName)

	return nil
}

func addTag(tagName, tagId, productId string, exl *vangogh_extracts.ExtractsList) error {
	addTagUrl := gog_urls.AddTag(productId, tagId)
	var artResp gog_types.AddRemoveTagResp
	if err := postResp(addTagUrl, &artResp); err != nil {
		return err
	}
	if !artResp.Success {
		return fmt.Errorf("vangogh: failed to add tag %s", tagName)
	}

	if err := exl.Add(vangogh_properties.TagIdProperty, productId, tagId); err != nil {
		return err
	}
	title, _ := exl.Get(vangogh_properties.TitleProperty, productId)
	fmt.Printf("added tag %s to %s (%s)\n", tagName, title, productId)

	return nil
}

func removeTag(tagName, tagId, productId string, exl *vangogh_extracts.ExtractsList) error {
	removeTagUrl := gog_urls.RemoveTag(productId, tagId)
	var artResp gog_types.AddRemoveTagResp
	if err := postResp(removeTagUrl, &artResp); err != nil {
		return err
	}
	if !artResp.Success {
		return fmt.Errorf("vangogh: failed to remove tag %s", tagName)
	}

	if err := exl.Remove(vangogh_properties.TagIdProperty, productId, tagId); err != nil {
		return err
	}
	title, _ := exl.Get(vangogh_properties.TitleProperty, productId)
	fmt.Printf("removed tag %s from %s (%s)\n", tagName, title, productId)

	return nil
}

func Tag(operation, tagName, id string) error {

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
		return addTag(tagName, tagId, id, exl)
	case removeOp:
		return removeTag(tagName, tagId, id, exl)
	default:
		return fmt.Errorf("vangogh: unknown tag operation %s", operation)
	}
}
