// @ts-check
// Note: type annotations allow type checking and IDEs autocompletion

const math = require("remark-math");
const katex = require("rehype-katex");

const lightCodeTheme = require('prism-react-renderer/themes/github');
const darkCodeTheme = require('prism-react-renderer/themes/dracula');

module.exports = {
    title: 'THUAI7 Docs',
    tagline: 'EESΛST 软件部 倾情奉献',
    favicon: 'img/favicon.ico',

    // Set the production url of your site here
    url: 'https://eesast.github.io/THUAI7/',
    // Set the /<baseUrl>/ pathname under which your site is served
    // For GitHub pages deployment, it is often '/<projectName>/'
    baseUrl: '/THUAI7/',

    // GitHub pages deployment config.
    // If you aren't using GitHub pages, you don't need these.
    organizationName: 'eesast', // Usually your GitHub org/user name.
    projectName: 'THUAI7', // Usually your repo name.

    onBrokenLinks: 'warn',
    onBrokenMarkdownLinks: 'warn',

    // Even if you don't use internalization, you can use this field to set useful
    // metadata like html lang. For example, if your site is Chinese, you may want
    // to replace "en" with "zh-Hans".
    i18n: {
        defaultLocale: 'zh-CN',
        locales: ['zh-CN'],
    },

    presets: [
        [
            'classic',
            /** @type {import('@docusaurus/preset-classic').Options} */
            ({
                docs: {
                    sidebarPath: require.resolve('./sidebars.js'),
                    editUrl:
                        'https://github.com/Panxuc/THUAI7-Docs/edit/main',
                    showLastUpdateAuthor: true,
                    showLastUpdateTime: true,
                    remarkPlugins: [math],
                    rehypePlugins: [katex],
                },
                blog: {
                    showReadingTime: true,
                },
                theme: {
                    customCss: require.resolve('./src/css/custom.css'),
                },
            }),
        ],
    ],

    stylesheets: [
        {
            href: 'https://cdn.jsdelivr.net/npm/katex@0.13.24/dist/katex.min.css',
            type: 'text/css',
            integrity:
                'sha384-odtC+0UGzzFL/6PNoE8rX/SPcQDXBJ+uRepguP4QkPCm2LBxH3FA3y+fKSiJ+AmM',
            crossorigin: 'anonymous',
        },
    ],

    themeConfig: {
        docs: {
            sidebar: {
                hideable: true,
            },
        },
        navbar: {
            title: 'THUAI7 Docs',
            logo: {
                alt: 'EESΛST Logo',
                src: 'img/favicon.ico',
            },
            items: [
                {
                    to: "/docs/category/规则",
                    activeBasePath: "docs",
                    label: "规则",
                    position: "left",
                    items: [
                        {
                            label: "引入",
                            to: "docs/intro",
                        },
                        {
                            label: "地图",
                            to: "docs/map",
                        },
                        {
                            label: "舰船",
                            to: "docs/ship",
                        },
                        {
                            label: "机制",
                            to: "docs/mechanics",
                        },
                        {
                            label: "接口",
                            to: "docs/interface",
                        },
                    ],
                },
                {
                    to: "/docs/category/常见问题",
                    activeBasePath: "docs",
                    label: "常见问题",
                    position: "left",
                    items: [
                        {
                            label: "常见问题",
                            to: "docs/faq",
                        },
                        {
                            label: "C++ 相关小知识",
                            to: "docs/faq/cpptips",
                        },
                    ],
                },
                { to: '/blog', label: '公告', position: 'left' },
                {
                    href: 'https://github.com/Panxuc/THUAI7-Docs',
                    label: 'GitHub',
                    position: 'right',
                },
                {
                    href: 'https://eesast.com',
                    label: 'EESΛST',
                    position: 'right',
                },
            ],
        },
        footer: {
            style: 'dark',
            links: [
                {
                    title: '规则',
                    items: [
                        {
                            label: "引入",
                            to: "docs/intro",
                        },
                        {
                            label: "地图",
                            to: "docs/map",
                        },
                        {
                            label: "舰船",
                            to: "docs/ship",
                        },
                        {
                            label: "机制",
                            to: "docs/mechanics",
                        },
                        {
                            label: "接口",
                            to: "docs/interface",
                        },
                    ],
                },
                {
                    title: '常见问题',
                    items: [
                        {
                            label: "常见问题",
                            to: "docs/faq",
                        },
                        {
                            label: "C++ 相关小知识",
                            to: "docs/faq/cpptips",
                        },
                    ],
                },
                {
                    title: "EESΛST",
                    items: [
                        {
                            label: "Home",
                            href: "https://eesast.com",
                        },
                        {
                            label: "Docs",
                            href: "https://docs.eesast.com",
                        },
                        {
                            label: "GitHub",
                            href: "https://github.com/eesast",
                        },
                        {
                            label: "THUAI7",
                            href: "https://github.com/eesast/THUAI7",
                        },
                    ],
                },
                {
                    title: '更多',
                    items: [
                        {
                            label: '公告',
                            to: 'blog',
                        },
                        {
                            label: 'GitHub',
                            href: 'https://github.com/Panxuc/THUAI7-Docs',
                        },
                    ],
                },
            ],
            logo: {
                alt: "EESΛST Logo",
                src: "img/favicon.ico",
                href: "https://eesast.com",
            },
            copyright: `Copyright © ${new Date().getFullYear()} THUAI7 Docs, EESΛST. Built with Docusaurus.`,
        },
        prism: {
            theme: lightCodeTheme,
            darkTheme: darkCodeTheme,
        },
    },

    plugins: [
        [
            require.resolve("@easyops-cn/docusaurus-search-local"),
            {
                hashed: true,
                language: ["en", "zh"],
            },
        ],
    ],
};
