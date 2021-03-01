package cmd

import (
	"encoding/gob"
	"encoding/json"
	"fmt"
	"github.com/arelate/gog_types"
	"github.com/arelate/vangogh_types"
	"github.com/arelate/vangogh_urls"
	"github.com/boggydigital/kvas"
	"io"
	"os"
)

const summaryFilename = "_summary.gob"

const (
	titleProp     = "title"
	developerProp = "developer"
	publisherProp = "publisher"
)

func Memorize(pt vangogh_types.ProductType, mt gog_types.Media, properties []string) error {
	if !vangogh_types.ValidProductType(pt) {
		return fmt.Errorf("vangogh: product type %s cannot be memorized", pt)
	}
	if !gog_types.ValidMedia(mt) {
		return fmt.Errorf("vangogh: media %s cannot be memorized", mt)
	}

	fmt.Printf("memorizing %s (%s)\n", pt, mt)

	var memorizeProductType func(io.Reader, []string) (map[string]string, error)

	switch pt {
	case vangogh_types.WishlistProducts:
		// WishlistProducts are the same underlying type as StoreProducts
		fallthrough
	case vangogh_types.StoreProducts:
		memorizeProductType = memorizeStoreProduct
	case vangogh_types.AccountProducts:
		memorizeProductType = memorizeAccountProduct
	case vangogh_types.Details:
		memorizeProductType = memorizeDetails
	case vangogh_types.ApiProductsV1:
		memorizeProductType = memorizeApiProductV1
	case vangogh_types.ApiProductsV2:
		memorizeProductType = memorizeApiProductV2
	default:
		return fmt.Errorf("vangogh: memorization of %s is not supported", pt)
	}

	dstProductType, err := vangogh_urls.DstProductTypeUrl(pt, mt)
	if err != nil {
		return err
	}
	kvProductType, err := kvas.NewJsonLocal(dstProductType)
	if err != nil {
		return err
	}

	memories := make(map[string]map[string]string, 0)

	for _, id := range kvProductType.All() {
		ptReader, err := kvProductType.Get(id)
		if err != nil {
			return err
		}
		memories[id], err = memorizeProductType(ptReader, properties)
		if err != nil {
			return err
		}
	}

	for _, prop := range properties {
		propertyMemories := make(map[string]string, 0)
		for _, id := range kvProductType.All() {
			propertyMemories[id] = memories[id][prop]
		}
		memoriesUrl, err := vangogh_urls.MemoriesUrl(pt, mt, prop)
		if err != nil {
			return err
		}

		if err := writeMemories(propertyMemories, memoriesUrl); err != nil {
			return err
		}
	}

	return nil
}

func writeMemories(memories map[string]string, memoriesUrl string) error {
	memoriesFile, err := os.Create(memoriesUrl)
	if err != nil {
		return err
	}
	defer memoriesFile.Close()

	return gob.NewEncoder(memoriesFile).Encode(memories)
}

func memorizeStoreProduct(spReader io.Reader, properties []string) (map[string]string, error) {
	memories := make(map[string]string, 0)
	var storeProduct *gog_types.StoreProduct
	if err := json.NewDecoder(spReader).Decode(&storeProduct); err != nil {
		return memories, err
	}

	for _, prop := range properties {
		memories[prop] = memorizeStoreProductProperty(storeProduct, prop)
	}

	return memories, nil
}

func memorizeStoreProductProperty(storeProduct *gog_types.StoreProduct, property string) string {
	if storeProduct == nil {
		return ""
	}
	switch property {
	case titleProp:
		return storeProduct.Title
	default:
		return ""
	}
}

func memorizeAccountProduct(spReader io.Reader, properties []string) (map[string]string, error) {
	memories := make(map[string]string, 0)
	var accountProduct *gog_types.AccountProduct
	if err := json.NewDecoder(spReader).Decode(&accountProduct); err != nil {
		return memories, err
	}

	for _, prop := range properties {
		memories[prop] = memorizeAccountProductProperty(accountProduct, prop)
	}

	return memories, nil
}

func memorizeAccountProductProperty(accountProduct *gog_types.AccountProduct, property string) string {
	if accountProduct == nil {
		return ""
	}
	switch property {
	case titleProp:
		return accountProduct.Title
	default:
		return ""
	}
}

func memorizeDetails(spReader io.Reader, properties []string) (map[string]string, error) {
	memories := make(map[string]string, 0)
	var details *gog_types.Details
	if err := json.NewDecoder(spReader).Decode(&details); err != nil {
		return memories, err
	}

	for _, prop := range properties {
		memories[prop] = memorizeDetailsProperty(details, prop)
	}

	return memories, nil
}

func memorizeDetailsProperty(details *gog_types.Details, property string) string {
	if details == nil {
		return ""
	}
	switch property {
	case titleProp:
		return details.Title
	default:
		return ""
	}
}

func memorizeApiProductV1(apv2Reader io.Reader, properties []string) (map[string]string, error) {
	memories := make(map[string]string, 0)
	var apiProductV1 *gog_types.ApiProductV1
	if err := json.NewDecoder(apv2Reader).Decode(&apiProductV1); err != nil {
		return memories, err
	}

	for _, prop := range properties {
		memories[prop] = memorizeApiProductV1Property(apiProductV1, prop)
	}

	return memories, nil
}

func memorizeApiProductV1Property(apiProductV1 *gog_types.ApiProductV1, property string) string {
	if apiProductV1 == nil {
		return ""
	}
	switch property {
	case titleProp:
		return apiProductV1.Title
	default:
		return ""
	}
}

func memorizeApiProductV2(apv2Reader io.Reader, properties []string) (map[string]string, error) {
	memories := make(map[string]string, 0)
	var apiProductV2 *gog_types.ApiProductV2
	if err := json.NewDecoder(apv2Reader).Decode(&apiProductV2); err != nil {
		return memories, err
	}

	for _, prop := range properties {
		memories[prop] = memorizeApiProductV2Property(apiProductV2, prop)
	}

	return memories, nil
}

func memorizeApiProductV2Property(apiProductV2 *gog_types.ApiProductV2, property string) string {
	if apiProductV2 == nil {
		return ""
	}
	switch property {
	case titleProp:
		return apiProductV2.Embedded.Product.Title
	default:
		return ""
	}
}
