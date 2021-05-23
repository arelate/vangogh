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

func Tag(operation, tagName, id string) error {
	var tagOpUrl *url.URL
	tagId := ""

	tagNameEx, err := vangogh_extracts.NewList(vangogh_properties.TagNameProperty)
	if err != nil {
		return err
	}

	if operation != createOp {
		tagIds := tagNameEx.Search(map[string][]string{vangogh_properties.TagNameProperty: {tagName}}, true)
		if len(tagIds) == 0 {
			return fmt.Errorf("vangogh: unknown tag-name %s", tagName)
		}
		if len(tagIds) > 1 {
			return fmt.Errorf("vangogh: ambiguous tag-name %s, matching tag-ids: %s",
				tagName,
				strings.Join(tagIds, ","))
		}
		tagId = tagIds[0]
	}

	switch operation {
	case createOp:
		tagOpUrl = gog_urls.CreateTag(tagName)
	case deleteOp:
		tagOpUrl = gog_urls.DeleteTag(tagId)
	case addOp:
		tagOpUrl = gog_urls.AddTag(id, tagId)
	case removeOp:
		tagOpUrl = gog_urls.RemoveTag(id, tagId)
	default:
		return fmt.Errorf("vangogh: unknown tag operation %s", operation)
	}

	httpClient, err := internal.HttpClient()
	if err != nil {
		return err
	}

	resp, err := httpClient.Post(tagOpUrl.String(), "", nil)
	if err != nil {
		return err
	}
	defer resp.Body.Close()

	if resp.StatusCode < 200 || resp.StatusCode > 299 {
		return fmt.Errorf("vangogh: unexpected status: %s", resp.Status)
	}

	jdec := json.NewDecoder(resp.Body)

	switch operation {
	case createOp:
		var ctResp gog_types.CreateTagResp
		if err := jdec.Decode(&ctResp); err != nil {
			return err
		}
		if ctResp.Id == "" {
			return fmt.Errorf("vangogh: invalid create tag response")
		}
		if err := tagNameEx.Add(vangogh_properties.TagNameProperty, ctResp.Id, tagName); err != nil {
			return err
		}
		fmt.Printf("created tag %s\n", tagName)
	case deleteOp:
		var dtResp gog_types.DeleteTagResp
		if err := jdec.Decode(&dtResp); err != nil {
			return err
		}
		if dtResp.Status != "deleted" {
			return fmt.Errorf("vangogh: invalid delete tag response")
		}
		if err := tagNameEx.Remove(vangogh_properties.TagNameProperty, tagId, tagName); err != nil {
			return err
		}
		fmt.Printf("deleted tag %s\n", tagName)
	case addOp:
		var artResp gog_types.AddRemoveTagResp
		if err := jdec.Decode(&artResp); err != nil {
			return err
		}
		if !artResp.Success {
			return fmt.Errorf("vangogh: failed to add tag %s", tagName)
		}
		exl, err := vangogh_extracts.NewList(
			vangogh_properties.TitleProperty,
			vangogh_properties.TagIdProperty,
		)
		if err != nil {
			return err
		}
		if err := exl.Add(vangogh_properties.TagIdProperty, id, tagId); err != nil {
			return err
		}
		title, ok := exl.Get(vangogh_properties.TitleProperty, id)
		if ok {
			fmt.Printf("added tag %s to %s (%s)\n", tagName, title, id)
		}
	case removeOp:
		var artResp gog_types.AddRemoveTagResp
		if err := jdec.Decode(&artResp); err != nil {
			return err
		}
		if !artResp.Success {
			return fmt.Errorf("vangogh: failed to remove tag %s", tagName)
		}
		exl, err := vangogh_extracts.NewList(
			vangogh_properties.TitleProperty,
			vangogh_properties.TagIdProperty,
		)
		if err != nil {
			return err
		}
		if err := exl.Remove(vangogh_properties.TagIdProperty, id, tagId); err != nil {
			return err
		}
		title, ok := exl.Get(vangogh_properties.TitleProperty, id)
		if ok {
			fmt.Printf("removed tag %s from %s (%s)\n", tagName, title, id)
		}
	}

	return nil
}
