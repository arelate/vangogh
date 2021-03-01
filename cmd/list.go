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

func List(ids []string, title string, pt vangogh_types.ProductType, mt gog_types.Media) error {

	titleMemoriesUrl, err := vangogh_urls.MemoriesUrl(pt, mt, "title")
	if err != nil {
		return err
	}

	// TODO: check if exists
	titleMemoriesFile, err := os.Open(titleMemoriesUrl)
	if err != nil {
		return err
	}
	defer titleMemoriesFile.Close()

	var titleMemories map[string]string

	if err := gob.NewDecoder(titleMemoriesFile).Decode(&titleMemories); err != nil {
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

		if titleMemory, ok := titleMemories[id]; ok {

			if title != "" && !strings.Contains(
				strings.ToLower(titleMemory),
				strings.ToLower(title)) {
				continue
			}

			fmt.Println(id, titleMemory)
		}
	}

	return nil
}
