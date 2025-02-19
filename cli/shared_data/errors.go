package shared_data

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/redux"
	"time"
)

const defaultErrorDurationDays = 7

var errorsDurationDays = map[vangogh_integration.ProductType]int{
	vangogh_integration.CatalogPage: 0, // this type errors are exceptional and most likely indicate transport or origin issue
	vangogh_integration.OrderPage:   0, // same as above
	vangogh_integration.AccountPage: 0, // same as above
	vangogh_integration.Details:     1, // same as above
}

func WriteTypeErrors(itemErrors map[string]error, rdx redux.Writeable) error {

	if len(itemErrors) == 0 {
		return nil
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

	var err error

	for property, idValues := range typeErrors {
		if err = rdx.BatchReplaceValues(property, idValues); err != nil {
			return err
		}
	}

	return nil
}

func SkipError(id string, productType vangogh_integration.ProductType, rdx redux.Writeable) (bool, error) {

	if dateStr, ok := rdx.GetLastVal(vangogh_integration.TypeErrorDateProperty, id); ok && dateStr != "" {
		if dt, err := time.Parse(time.RFC3339, dateStr); err == nil {

			var errorDuration time.Duration
			if edd, sure := errorsDurationDays[productType]; sure {
				errorDuration = time.Duration(edd)
			} else {
				errorDuration = defaultErrorDurationDays
			}

			errorExpires := dt.Add(errorDuration * 24 * time.Hour)

			if time.Now().UTC().Before(errorExpires) {

				nod.Log("skipping current %s error last encountered: %s", productType, dateStr)

				return true, nil
			} else {

				nod.Log("clearing %s error last encountered: %s", productType, dateStr)

				if err = rdx.CutKeys(vangogh_integration.TypeErrorDateProperty, id); err != nil {
					return false, err
				}
				if err = rdx.CutKeys(vangogh_integration.TypeErrorMessageProperty, id); err != nil {
					return false, err
				}

				return false, nil
			}

		} else {
			return false, err
		}
	}

	return false, nil
}
