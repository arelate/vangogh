package jsonsha

import (
	"crypto/sha256"
	"encoding/json"
	"fmt"
)

func Marshal(data interface{}) ([]byte, string, error) {
	bytes, err := json.Marshal(data)
	if err != nil {
		return nil, "", err
	}

	h := sha256.New()
	_, err = h.Write(bytes)
	if err != nil {
		return nil, "", err
	}
	bs := h.Sum(nil)
	hash := fmt.Sprintf("%x", bs)

	return bytes, hash, nil
}
