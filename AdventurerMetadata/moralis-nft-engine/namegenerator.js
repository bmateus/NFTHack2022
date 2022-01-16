

const name_parts = [
	"xi",
	"xin",
	"tal",
	"bo",
	"bal",
	"bi",
	"ain",
	"el",
	"jor",
	"kal",
	"asci",
	"xian",
	"aeon",
	"flux",
	"vol",
	"tar",
	"jin",
	"rax",
	"zer",
	"zol",
	"kha",
	"pho",
	"dei",
	"bos",
	"al",
	"rex",
	"gi",
	"mar",
	"ux",
	"or",
	"gri",
	"kil",
	"s's",
	"qi",
	"qua",
	"zin",
	"kor",
	"lin",
	"hy",
	"ruz"
]

const titles = [
	"Bloodthirsty",
	"Shameless",
	"Unwise",
	"Traitor",
	"Enlightened",
	"Perverted",
	"Seducer",
	"Concubine",
	"Weak",
	"Degenerate",
	"Gambler",
	"Bastard",
	"Builder",
	"Visionary",
	"Conquerer",
	"Cruel",
	"Vaporizor",
	"Fertile",
]

const generateAlienName = () => {

	const first = Math.floor(Math.random() * name_parts.length);
	const second = Math.floor(Math.random() * name_parts.length);
	const fullname = name_parts[first]+name_parts[second];
	return fullname.charAt(0).toUpperCase() + fullname.slice(1);
}

module.exports = {
	generateAlienName,
	titles
}

//console.log(generateAlienName());