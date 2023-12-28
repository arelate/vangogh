package vets

import (
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/nod"
	"strings"
)

func IndexOnly(fix bool) error {

	ioa := nod.NewProgress("checking index only product data...")
	defer ioa.End()

	pts := vangogh_local_data.LocalProducts()
	ioa.TotalInt(len(pts))

	for _, pt := range pts {

		vr, err := vangogh_local_data.NewReader(pt)
		if err != nil {
			return ioa.EndWithError(err)
		}

		tpw := nod.NewProgress(" %s", pt)
		iop, err := vr.VetIndexOnly(fix, tpw)
		if err != nil {
			return ioa.EndWithError(err)
		}

		tpw.EndWithResult(indexVetResult(iop))
		ioa.Increment()
	}

	ioa.EndWithResult("done")

	rioa := nod.NewProgress("checking index only redux properties...")
	defer rioa.End()

	rdx, err := vangogh_local_data.NewReduxVetter(vangogh_local_data.ReduxProperties()...)
	if err != nil {
		return rioa.EndWithError(err)
	}

	if riop, err := rdx.VetIndexOnly(fix, rioa); err != nil {
		return rioa.EndWithError(err)
	} else {
		rioa.EndWithResult(indexVetResult(riop))
	}

	return nil
}

func indexVetResult(found []string) string {
	result := "done"
	if len(found) > 0 {
		result = "found: " + strings.Join(found, ",")
	}
	return result
}
