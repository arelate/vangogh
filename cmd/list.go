package cmd

import (
	"encoding/gob"
	"fmt"
	"github.com/arelate/gog_types"
	"github.com/arelate/vangogh_types"
	"github.com/arelate/vangogh_urls"
	"github.com/boggydigital/kvas"
	"os"
	"strings"
)

type productTitle struct {
	Title string `json:"title"`
}

func List(ids []string, title string, productType, media string) error {
	pt := vangogh_types.ParseProductType(productType)
	mt := gog_types.ParseMedia(media)

	summaryPath := "metadata/_summary.gob"
	// TODO: check if exists
	summaryFile, err := os.Open(summaryPath)
	if err != nil {
		return err
	}
	defer summaryFile.Close()

	var summary map[string]map[string]string

	if err := gob.NewDecoder(summaryFile).Decode(&summary); err != nil {
		return err
	}

	dstUrl, err := vangogh_urls.DstProductTypeUrl(pt, mt)
	if err != nil {
		return err
	}

	kv, err := kvas.NewJsonLocal(dstUrl)
	if err != nil {
		return err
	}

	if len(ids) == 0 {
		ids = kv.All()
	}

	for _, id := range ids {

		if sum, ok := summary[id]; ok {

			sTitle := sum["title"]

			if title != "" && !strings.Contains(
				strings.ToLower(sTitle),
				strings.ToLower(title)) {
				continue
			}

			fmt.Println(id, sTitle)
		}
	}

	return nil
}
