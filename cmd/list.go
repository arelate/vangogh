package cmd

import (
	"encoding/gob"
	"fmt"
	"github.com/arelate/gog_types"
	"github.com/arelate/vangogh_types"
	"github.com/arelate/vangogh_urls"

	"os"
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

	//developerMemories := make(map[string]string, 0)
	//publisherMemories := make(map[string]string, 0)
	//
	//titleMemories, err := loadMemories(pt, mt, vangogh_types.TitleProperty)
	//if err != nil {
	//	return err
	//}
	//
	//if vangogh_types.SupportsProperty(pt, vangogh_types.DeveloperProperty) {
	//	developerMemories, err = loadMemories(pt, mt, vangogh_types.DeveloperProperty)
	//	if err != nil {
	//		return err
	//	}
	//}
	//
	//if vangogh_types.SupportsProperty(pt, vangogh_types.PublisherProperty) {
	//	publisherMemories, err = loadMemories(pt, mt, vangogh_types.PublisherProperty)
	//	if err != nil {
	//		return err
	//	}
	//}
	//
	//vr, err := vangogh_values.NewReader(pt, mt)
	//if err != nil {
	//	return err
	//}
	//
	//if len(ids) == 0 {
	//	ids = vr.All()
	//}
	//
	//for _, id := range ids {
	//
	//	titleMemory, ok := titleMemories[id]
	//	if !ok {
	//		continue
	//	}
	//
	//	devMemory := developerMemories[id]
	//	pubMemory := publisherMemories[id]
	//
	//	if title != "" &&
	//		titleMemory != "" &&
	//		!strings.Contains(strings.ToLower(titleMemory),strings.ToLower(title)) {
	//		continue
	//	}
	//
	//	if developer != "" &&
	//		devMemory != "" &&
	//		!strings.Contains(strings.ToLower(devMemory), strings.ToLower(developer)) {
	//		continue
	//	}
	//
	//	if publisher != "" &&
	//		pubMemory != "" &&
	//		!strings.Contains(strings.ToLower(pubMemory), strings.ToLower(publisher)) {
	//		continue
	//	}
	//
	//	fmt.Printf("%s %s", id, titleMemory)
	//	if developer != "" && devMemory != "" {
	//		fmt.Printf(", Developer:%s", devMemory)
	//	}
	//	if publisher != "" && pubMemory != "" {
	//		fmt.Printf(", Publisher:%s", pubMemory)
	//	}
	//	fmt.Println()
	//}

	return nil
}
