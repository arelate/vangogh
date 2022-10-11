package dirs

import "github.com/arelate/vangogh_local_data"

func SetStateDir(d string) {
	vangogh_local_data.ChRoot(d)
}

func SetTempDir(d string) {
	vangogh_local_data.SetTempDir(d)
}
