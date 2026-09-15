module.exports = function (eleventyConfig) {
  eleventyConfig.addPassthroughCopy("style");
  eleventyConfig.addPassthroughCopy("images");
  eleventyConfig.addPassthroughCopy("js");
  eleventyConfig.addPassthroughCopy({
    "node_modules/bulma/css/bulma.min.css": "style/vendor/bulma.min.css"
  });

  eleventyConfig.addFilter("readableDate", (dateObj) => {
    if (!dateObj) return "";
    return new Date(dateObj).toISOString().slice(0, 10);
  });

  return {
    dir: {
      input: ".",
      includes: "_includes",
      data: "_data",
      output: "_site"
    }
  };
};
