package cli

import (
	"io"
	"net/url"
	"os"
	"path/filepath"
	"strings"

	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/cli/reqs"
	"github.com/boggydigital/camino"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
)

func GetDescriptionImagesHandler(u *url.URL) error {
	since, err := vangogh_integration.SinceHouseAgoFromUrl(u)
	if err != nil {
		return nil
	}

	q := u.Query()

	ids, err := vangogh_integration.IdsFromUrl(u)
	if err != nil {
		return err
	}

	all := q.Has(vangogh_integration.UrlAllParameter)
	force := q.Has(vangogh_integration.UrlForceParameter)

	return GetDescriptionImages(ids, since, all, force)
}

func GetDescriptionImages(ids []string, since int64, all, force bool) error {

	gdia := nod.NewProgress("getting description images...")
	defer gdia.Done()

	metadataDir := camino.GetAbs(vangogh_integration.Metadata)

	descOverviewDir := filepath.Join(metadataDir, vangogh_integration.GogDescriptionOverviewKeyValues)
	kvDescOverview, err := kevlar.New(descOverviewDir, kevlar.TxtExt)
	if err != nil {
		return err
	}

	descFeaturesDir := filepath.Join(metadataDir, vangogh_integration.GogDescriptionFeaturesKeyValues)
	kvDescFeatures, err := kevlar.New(descFeaturesDir, kevlar.TxtExt)
	if err != nil {
		return err
	}

	gogApiProductsDir := vangogh_integration.AbsProductTypeDir(vangogh_integration.GogApiProducts)

	kvGogApiProducts, err := kevlar.New(gogApiProductsDir, kevlar.JsonExt)
	if err != nil {
		return err
	}

	if len(ids) == 0 {
		if all {
			since = -1
		}
		newApiProducts := kvGogApiProducts.Since(since, kevlar.Create, kevlar.Update)
		for id := range newApiProducts {
			ids = append(ids, id)
		}
	}

	gdia.TotalInt(len(ids))

	for _, id := range ids {

		var descriptionImages []string

		var descOverviewBytes []byte
		descOverviewBytes, err = getDescBytes(id, kvDescOverview)
		if err != nil {
			return err
		}

		if len(descOverviewBytes) > 0 {
			descriptionImages = vangogh_integration.ExtractDescItems(string(descOverviewBytes))
		}

		var descFeaturesBytes []byte
		descFeaturesBytes, err = getDescBytes(id, kvDescFeatures)
		if err != nil {
			return err
		}

		if len(descFeaturesBytes) > 0 {
			descriptionImages = append(descriptionImages, vangogh_integration.ExtractDescItems(string(descFeaturesBytes))...)
		}

		if len(descriptionImages) == 0 {
			gdia.Increment()
			continue
		}

		for _, descriptionImageUrl := range descriptionImages {
			descriptionImageUrl = strings.Replace(descriptionImageUrl, "\n", "", -1)
			if err = getDescriptionImage(descriptionImageUrl, force); err != nil {
				gdia.Error(err)
			}
		}

		gdia.Increment()
	}

	return nil
}

func getDescBytes(id string, keyValues kevlar.KeyValues) ([]byte, error) {

	if !keyValues.Has(id) {
		return nil, nil
	}

	rcValue, err := keyValues.Get(id)
	if err != nil {
		return nil, err
	}
	defer rcValue.Close()

	return io.ReadAll(rcValue)
}

func getDescriptionImage(descriptionImageUrl string, force bool) error {

	diu, err := url.Parse(descriptionImageUrl)
	if err != nil {
		return err
	}

	gdia := nod.NewProgress(" - %s", diu.Path)
	defer gdia.Done()

	adip, err := vangogh_integration.AbsDescriptionImagePath(diu.Path)
	if err != nil {
		return err
	}

	adiDir, _ := filepath.Split(adip)
	if _, err = os.Stat(adiDir); os.IsNotExist(err) {
		if err = os.MkdirAll(adiDir, camino.DefaultFileMode); err != nil {
			return err
		}
	}

	dc := reqs.GetDoloClient()

	if err = dc.Download(diu, force, gdia, adip); err != nil {
		if ext := filepath.Ext(adip); ext != "" && strings.Contains(err.Error(), "404") {
			nod.Log(" - %s not found", gdia)
			return nil
		}
		return err
	}

	return nil
}
