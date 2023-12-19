package vets

import (
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/nod"
)

func IndexMissing(fix bool) error {

	ima := nod.NewProgress("checking index missing product data...")
	defer ima.End()

	pts := vangogh_local_data.LocalProducts()
	ima.TotalInt(len(pts))

	for _, pt := range pts {

		vr, err := vangogh_local_data.NewReader(pt)
		if err != nil {
			return ima.EndWithError(err)
		}

		tpw := nod.NewProgress(" %s", pt)
		iop, err := vr.VetIndexMissing(fix, tpw)
		if err != nil {
			return ima.EndWithError(err)
		}

		tpw.EndWithResult(indexVetResult(iop))
		ima.Increment()
	}

	ima.EndWithResult("done")

	rima := nod.NewProgress("checking index missing redux properties...")
	defer rima.End()

	rdx, err := vangogh_local_data.ReduxVetter(vangogh_local_data.ReduxProperties()...)
	if err != nil {
		return rima.EndWithError(err)
	}

	if rimp, err := rdx.VetIndexMissing(fix, rima); err != nil {
		return rima.EndWithError(err)
	} else {
		rima.EndWithResult(indexVetResult(rimp))
	}

	return nil
}
