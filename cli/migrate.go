package cli

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/pathways"
	"net/url"
)

func MigrateHandler(_ *url.URL) error {
	return Migrate()
}

func Migrate() error {
	ma := nod.Begin("migrating data...")
	defer ma.Done()

	productTypes := vangogh_integration.LocalProducts()
	productTypes = append(productTypes, vangogh_integration.GOGPagedProducts()...)
	productTypes = append(productTypes, vangogh_integration.GOGArrayProducts()...)
	productTypes = append(productTypes, vangogh_integration.SteamArrayProducts()...)

	for _, pt := range productTypes {
		absPtDir, err := vangogh_integration.AbsProductTypeDir(pt)
		if err != nil {
			return err
		}

		if err := kevlar.Migrate(absPtDir); err != nil {
			return err
		}
	}

	reduxDir, err := pathways.GetAbsRelDir(vangogh_integration.Redux)
	if err != nil {
		return err
	}

	if err := kevlar.Migrate(reduxDir); err != nil {
		return err
	}

	return nil
}
