package hash

import (
	"crypto/sha256"
	"encoding/json"
	"fmt"
)

func Sha256(data interface{}) (string, error) {
	bytes, err := json.Marshal(data)
	if err != nil {
		return "", err
	}

	h := sha256.New()
	_, err = h.Write(bytes)
	if err != nil {
		return "", err
	}
	return fmt.Sprintf("%x", h.Sum(nil)), nil
}
