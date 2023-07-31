package cli

import (
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/packer"
	"net/url"
)

func ExportHandler(_ *url.URL) error {
	return Export(vangogh_local_data.AbsTempDir())
}

func Export(to string) error {

	root := vangogh_local_data.Pwd()
	from := vangogh_local_data.AbsMetadataDir()

	ea := nod.NewProgress("exporting metadata...")
	defer ea.End()

	if err := packer.Pack(root, from, to, ea); err != nil {
		return ea.EndWithError(err)
	}

	ea.EndWithResult("done")

	return nil
}
