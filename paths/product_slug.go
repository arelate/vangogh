package paths

func ProductSlug(slug string) string {
	return ProductPath + "?slug=" + slug
}
