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
	"path"
)

const summaryFilename = "_summary.gob"

const (
	titleProp     = "title"
	developerProp = "developer"
	publisherProp = "publisher"
)

type product struct {
	Title string `json:"title"`
}

func Summarize(productType, media string) error {
	pt := vangogh_types.ParseProductType(productType)
	mt := gog_types.ParseMedia(media)

	fmt.Printf("summarizing %s (%s)\n", productType, media)

	dstSummary := "metadata"
	summaryPath := path.Join(dstSummary, summaryFilename)

	var summary map[string]map[string]string

	if _, err := os.Stat(summaryPath); !os.IsNotExist(err) {
		summaryFile, err := os.Open(summaryPath)
		if err != nil {
			return err
		}
		if err := gob.NewDecoder(summaryFile).Decode(&summary); err != nil {
			return err
		}
	}

	if summary == nil {
		summary = make(map[string]map[string]string, 0)
	}

	var summarizeProductType func(io.Reader, []string) (map[string]string, error)

	switch pt {
	case vangogh_types.StoreProducts:
		fallthrough
	case vangogh_types.WishlistProducts:
		fallthrough
	case vangogh_types.Details:
		fallthrough
	case vangogh_types.AccountProducts:
		summarizeProductType = summarizeProduct
	case vangogh_types.ApiProducts:
		summarizeProductType = summarizeApiProduct
	default:
		return fmt.Errorf("vangogh: summarization of %s is not supported", pt)
	}

	dstProductType, err := vangogh_urls.DstProductTypeUrl(pt, mt)
	if err != nil {
		return err
	}
	kvProductType, err := kvas.NewJsonLocal(dstProductType)
	if err != nil {
		return err
	}

	for _, id := range kvProductType.All() {
		if _, ok := summary[id]; ok {
			// don't reindex unless requested
			continue
		}
		if summary[id] == nil {
			summary[id] = make(map[string]string, 0)
		}
		ptReadCloser, err := kvProductType.Get(id)
		if err != nil {
			return err
		}
		productTypeSummary, err := summarizeProductType(ptReadCloser, []string{titleProp})
		for k, v := range productTypeSummary {
			if v != "" {
				summary[id][k] = v
			}
		}
		ptReadCloser.Close()
	}

	summaryFile, err := os.Create(summaryPath)
	if err != nil {
		return err
	}
	defer summaryFile.Close()

	if err := gob.NewEncoder(summaryFile).Encode(summary); err != nil {
		return err
	}

	return nil
}

func summarizeProduct(spReader io.Reader, props []string) (map[string]string, error) {
	var prod product
	summary := make(map[string]string, 0)
	if err := json.NewDecoder(spReader).Decode(&prod); err != nil {
		return summary, err
	}

	for _, prop := range props {
		switch prop {
		case titleProp:
			summary[prop] = prod.Title
		default:
			return summary, fmt.Errorf("vangogh: property %s not supported for summarization", prop)
		}
	}

	return summary, nil
}

func summarizeApiProduct(apReader io.Reader, props []string) (map[string]string, error) {
	var apiProd gog_types.ApiProduct
	summary := make(map[string]string, 0)
	if err := json.NewDecoder(apReader).Decode(&apiProd); err != nil {
		return summary, err
	}

	for _, prop := range props {
		switch prop {
		case titleProp:
			summary[prop] = apiProd.Embedded.Product.Title
		default:
			return summary, fmt.Errorf("vangogh: property %s not supported for summarization", prop)
		}
	}

	return summary, nil
}
