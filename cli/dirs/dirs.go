package dirs

import "github.com/arelate/vangogh_local_data"

var LogsDir = ""

func SetStateDir(d string) {
	vangogh_local_data.ChRoot(d)
}

func SetTempDir(d string) {
	vangogh_local_data.SetTempDir(d)
}

func SetLogsDir(d string) {
	LogsDir = d
}
