package cmd

import (
	"fmt"
	"github.com/arelate/gog_urls"
	"github.com/arelate/vangogh_extracts"
	"github.com/arelate/vangogh_properties"
	"github.com/boggydigital/vangogh/internal"
	"net/url"
	"strings"
)

func Tag(operation, tagName, id string) error {
	var tagOpUrl *url.URL
	tagId := ""

	if operation != "create" {
		tagNameEx, err := vangogh_extracts.NewList(vangogh_properties.TagNameProperty)
		if err != nil {
			return err
		}
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
	case "create":
		tagOpUrl = gog_urls.CreateTag(tagName)
	case "delete":
		tagOpUrl = gog_urls.DeleteTag(tagId)
	case "add":
		tagOpUrl = gog_urls.AddTag(id, tagId)
	case "remove":
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

	//fmt.Println(operation, tagName, id)
	//fmt.Println(resp.Body)

	return nil
}
