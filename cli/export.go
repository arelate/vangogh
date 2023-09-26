package cli

import (
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/packer"
	"net/url"
	"path/filepath"
)

func ExportHandler(_ *url.URL) error {
	return Export(vangogh_local_data.AbsOutputFilesDir())
}

func Export(to string) error {

	from := vangogh_local_data.AbsMetadataDir()
	root, _ := filepath.Split(from)

	ea := nod.NewProgress("exporting metadata...")
	defer ea.End()

	if err := packer.Pack(root, from, to, ea); err != nil {
		return ea.EndWithError(err)
	}

	ea.EndWithResult("done")

	return nil
}
