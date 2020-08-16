package storage

import (
	"io/ioutil"
	"os"
	"path"
)

func Save(bytes []byte, filename string) error {

	dir := path.Dir(filename)
	if _, err := os.Stat(dir); os.IsNotExist(err) {
		os.MkdirAll(dir, 0755)
	}

	return ioutil.WriteFile(filename, bytes, 0644)

	return nil
}
