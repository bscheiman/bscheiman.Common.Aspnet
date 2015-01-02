var time = {
    update: function() {
        $("time").each(function() {
            var t = $(this);
            var m = moment.utc(t.attr("datetime") * 1000).local();

            t.html(m.format(t.attr("data-format")));
        });
    }
}