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

func loadMemories(pt vangogh_types.ProductType, mt gog_types.Media, property string) (map[string]string, error) {
	memories := make(map[string]string, 0)

	memoriesUrl, err := vangogh_urls.MemoriesUrl(pt, mt, property)
	if err != nil {
		return memories, err
	}

	if _, err := os.Stat(memoriesUrl); os.IsNotExist(err) {
		return memories, fmt.Errorf("vangogh: no memories for product type %s, media %s, property %s", pt, mt, property)
	}

	memoriesFile, err := os.Open(memoriesUrl)
	if err != nil {
		return memories, err
	}
	defer memoriesFile.Close()

	if err := gob.NewDecoder(memoriesFile).Decode(&memories); err != nil {
		return memories, err
	}

	return memories, nil
}

func List(ids []string, pt vangogh_types.ProductType, mt gog_types.Media, title, developer, publisher string) error {

	titleMemories, err := loadMemories(pt, mt, vangogh_types.TitleProperty)
	if err != nil {
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
