package shared_data

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/pathways"
	"github.com/boggydigital/redux"
	"os"
	"path/filepath"
	"time"
)

type PropertyIdValues map[string]map[string][]string

func InitReductions(properties ...string) PropertyIdValues {
	piv := make(PropertyIdValues)
	for _, property := range properties {
		piv[property] = make(map[string][]string)
	}
	return piv
}

func WriteReductions(rdx redux.Writeable, piv PropertyIdValues) error {
	for property, keyValues := range piv {
		if err := rdx.BatchReplaceValues(property, keyValues); err != nil {
			return err
		}
	}
	return nil
}

func WriteTypeErrors(productType vangogh_integration.ProductType, itemErrors map[string]error) error {

	perTypeReduxDir, err := pathways.GetAbsRelDir(vangogh_integration.PerTypeRedux)
	if err != nil {
		return err
	}

	typeReduxDir := filepath.Join(perTypeReduxDir, productType.String())

	if _, err = os.Stat(typeReduxDir); os.IsNotExist(err) {
		if err = os.MkdirAll(typeReduxDir, 0755); err != nil {
			return err
		}
	}

	rdx, err := redux.NewWriter(typeReduxDir, vangogh_integration.TypeErrorProperties()...)
	if err != nil {
		return err
	}

	typeErrors := make(PropertyIdValues)
	for _, property := range vangogh_integration.TypeErrorProperties() {
		typeErrors[property] = make(map[string][]string)
	}

	for id, typeErr := range itemErrors {
		var value string
		for _, property := range vangogh_integration.TypeErrorProperties() {
			switch property {
			case vangogh_integration.TypeErrorMessageProperty:
				value = typeErr.Error()
			case vangogh_integration.TypeErrorDateProperty:
				value = time.Now().UTC().Format(time.RFC3339)
			}
			typeErrors[property][id] = []string{value}
		}
	}

	for property, idValues := range typeErrors {
		if err = rdx.BatchReplaceValues(property, idValues); err != nil {
			return err
		}
	}

	return nil
}
