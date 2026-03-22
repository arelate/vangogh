package cli

import (
	"io"
	"net/url"
	"os"
	"path/filepath"
	"strings"

	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/cli/reqs"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/pathways"
)

func GetDescriptionImagesHandler(u *url.URL) error {
	since, err := vangogh_integration.SinceFromUrl(u)
	if err != nil {
		return nil
	}

	q := u.Query()

	ids, err := vangogh_integration.IdsFromUrl(u)
	if err != nil {
		return err
	}

	all := q.Has("all")
	force := q.Has("force")

	return GetDescriptionImages(ids, since, all, force)
}

func GetDescriptionImages(ids []string, since int64, all, force bool) error {

	gdia := nod.NewProgress("getting description images...")
	defer gdia.Done()

	metadataDir := vangogh_integration.Pwd.AbsDirPath(vangogh_integration.Metadata)

	descOverviewDir := filepath.Join(metadataDir, vangogh_integration.DescriptionOverviewKeyValues)
	kvDescOverview, err := kevlar.New(descOverviewDir, kevlar.TxtExt)
	if err != nil {
		return err
	}

	descFeaturesDir := filepath.Join(metadataDir, vangogh_integration.DescriptionFeaturesKeyValues)
	kvDescFeatures, err := kevlar.New(descFeaturesDir, kevlar.TxtExt)
	if err != nil {
		return err
	}

	apiProductsDir, err := vangogh_integration.AbsProductTypeDir(vangogh_integration.ApiProducts)
	if err != nil {
		return err
	}

	kvApiProducts, err := kevlar.New(apiProductsDir, kevlar.JsonExt)
	if err != nil {
		return err
	}

	if len(ids) == 0 {
		if all {
			since = -1
		}
		newApiProducts := kvApiProducts.Since(since, kevlar.Create, kevlar.Update)
		for id := range newApiProducts {
			ids = append(ids, id)
		}
	}

	gdia.TotalInt(len(ids))

	for _, id := range ids {

		var descriptionImages []string

		var descOverviewBytes []byte
		descOverviewBytes, err = getDescBytes(id, vangogh_integration.DescriptionOverviewKeyValues, kvDescOverview)
		if err != nil {
			return err
		}

		if len(descOverviewBytes) > 0 {
			descriptionImages = vangogh_integration.ExtractDescItems(string(descOverviewBytes))
		}

		var descFeaturesBytes []byte
		descFeaturesBytes, err = getDescBytes(id, vangogh_integration.DescriptionFeaturesKeyValues, kvDescFeatures)
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

func getDescBytes(id, property string, keyValues kevlar.KeyValues) ([]byte, error) {

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
		if err = os.MkdirAll(adiDir, pathways.PermUrwGrwOr); err != nil {
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
